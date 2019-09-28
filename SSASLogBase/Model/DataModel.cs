using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSASLogBase.Models
{
    /// <summary>
    /// What's still missing, is the tables collection in the refresh object.
    /// This will be needed when we do not process a database at once, but 
    /// process individual tables.
    /// </summary>

    public class SSASServer
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ICollection<SSASDatabase> SSASDatabases { get; set; }
    }

    public class SSASDatabase
    {
        public Guid ID { get; set; }
        public string Name;
        public ICollection<Refresh> Refreshes { get; set; }

        // Navigation properties
        public SSASServer SSASServer { get; set; }
    }

    public class Refresh
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid ID { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [DefaultValue(null)]  // accept the enum value as 0 too 
        public RefreshType RefreshType { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        [DefaultValue(null)]  // accept the enum value as 0 too 
        public RefreshStatus RefreshStatus { get; set; }

        public ICollection<Message> Messages { get; set; }

        // Navigation properties
        public SSASDatabase Database { get; set; }
    }

    public class Message
    {
        public Guid ID { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }
        public Location Location { get; set; }

        // Navigation properties
        public Refresh Refresh { get; set; }

    }

    public class Location
    {
        public Guid ID { get; set; }
        public SourceObject SourceObject { get; set; }
        public Guid MessagId { get; set; }

        // Navigation properties
        public Message Message { get; set; }

    }

    public class SourceObject
    {
        public Guid ID { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public Guid LocationId { get; set; }

        // Navigation properties
        public Location Location { get; set; }
    }

    public enum RefreshType
    {
        [EnumMember(Value = "Full")]
        Full
    }

    public enum RefreshStatus
    {
        [EnumMember(Value = "Succeeded")]
        Succeeded,
        [EnumMember(Value = "InProgress")]
        InProgress,
        [EnumMember(Value = "Cancelled")]
        Cancelled,
        [EnumMember(Value = "Failed")]
        Failed
    }
}
