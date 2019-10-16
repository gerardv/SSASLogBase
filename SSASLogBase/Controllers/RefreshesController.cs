using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SSASLogBase.Data;
using SSASLogBase.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SSASLogBase.Controllers
{
    //[Authorize]
    public class RefreshesController : Controller
    {
        private readonly DataContext _context;

        public RefreshesController(DataContext context)
        {
            _context = context;
        }

        // GET: Refreshes
        public async Task<IActionResult> Index(int page = 0)
        {
            // Todo: Add server / model filter to View

            return View(await _context.Refreshes
                .Include("Database")
                .Include("Database.SSASServer")
                .Include("Messages")
                .Skip( page ) // Todo: implement pagination in View.
                .Take(10) // Todo: make this a parameter?
                .OrderByDescending(r => r.StartTime)
                .ToListAsync());
        }

        // GET: Refreshes/Details/5
        public async Task<IActionResult> Details(Guid id, string server, string model)
        {
            AuthenticationResult result = null;
            HttpClient client = new HttpClient();
            JsonSerializerSettings settings = new JsonSerializerSettings();

            if (id.ToString() == "00000000-0000-0000-0000-000000000000"
                || server == null
                || model == null)
            {
                return BadRequest();
            }

            Refresh refresh = await _context.Refreshes
                .Include("Database")
                .Include("Database.SSASServer")
                .Include("Messages")
                .FirstOrDefaultAsync(l => l.ID == id);

            if (refresh == null)
            {
                // Refresh has not been retrieved yet... trying to get it from Azure

                // First set the authorization context
                AuthenticationContext authContext = new AuthenticationContext(AzureAdOptions.Settings.Authority, new NaiveSessionCache(AzureAdOptions.Settings.ClientId, HttpContext.Session));
                ClientCredential credential = new ClientCredential(AzureAdOptions.Settings.ClientId, AzureAdOptions.Settings.ClientSecret);

                // Todo: Check whether server is online

                // Using ADAL.Net, get a bearer token to access the Azure Analysis Service
                result = await authContext.AcquireTokenAsync(AzureAdOptions.Settings.ServerAudience, credential);

                // Set and fire the request to get the AS server's details, including it's current status
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, AzureAdOptions.Settings.ServerBaseAddress + server + "?" + AzureAdOptions.Settings.ServerAPIVersion);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage serverResponse = await client.SendAsync(request);

                // If that failed, return an error message
                if ( ! serverResponse.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Something went terribly wrong... I cannot help you at this moment.";
                    return View("PlainFeedback");
                }

                // Parse the received server details
                string serverResponseString = await serverResponse.Content.ReadAsStringAsync();
                ServerProperties serverProperties = JsonConvert.DeserializeObject<ServerProperties>(serverResponseString, settings);

                // If the server's status does not equal 'Succeeded', return an error message.
                if ( serverProperties.properties.state != "Succeeded")
                {
                    ViewBag.Message = "The server is not up and running, so I cannot retrieve the requested refresh details. Start the server or wait for it to finishing resuming or scaling and try again.";
                    return View("PlainFeedback");
                }

                // Now we get a bearer token to access the Azure Analysis Service
                result = await authContext.AcquireTokenAsync(AzureAdOptions.Settings.ModelAudience, credential);

                // Set and fire the request to get the AS model's refresh details
                request = new HttpRequestMessage(HttpMethod.Get, AzureAdOptions.Settings.ModelBaseAddress + server + "/models/"+ model + "/refreshes/" +  id);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = await client.SendAsync(request);

                // We can (and should) now dispose of the http client.
                client.Dispose();

                if ( response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    try
                    {
                        refresh = JsonConvert.DeserializeObject<Refresh>(responseString, settings);

                        SSASServer _server = _context.Servers.FirstOrDefault(s => s.Name == server);
                        if (_server == null) _server = new SSASServer()
                        {
                            Name = server
                        };
                        SSASDatabase _database = _context.Databases.FirstOrDefault(d => d.Name == model);
                        if (_database == null) _database = new SSASDatabase()
                        {
                            Name = model,
                            SSASServer = _server
                        };

                        refresh.ID = id;
                        refresh.Database = _database;

                        _context.Refreshes.Add(refresh);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    ViewBag.Message = "Something went terribly wrong, right at the end... I cannot help you at this moment.";
                    return View("PlainFeedback");
                }
            }

            return View(refresh);
        }

        // GET: Refreshes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var refresh = await _context.Refreshes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (refresh == null)
            {
                return NotFound();
            }

            return View(refresh);
        }

        // POST: Refreshes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var refresh = await _context.Refreshes.FindAsync(id);
            _context.Refreshes.Remove(refresh);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RefreshExists(Guid id)
        {
            return _context.Refreshes.Any(e => e.ID == id);
        }
    }
}
