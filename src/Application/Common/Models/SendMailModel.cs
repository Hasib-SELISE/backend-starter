namespace Application.Common.Models;

public class SendMailModel
{
    public string Purpose { get; set; }

    public string Language { get; set; }
    
    public IEnumerable<string> To { get; set; }
    
    public IEnumerable<string> Bcc { get; set; }
    
    public IEnumerable<string> Cc { get; set; }

    public IEnumerable<string> Attachments { get; set; }

    public IEnumerable<string> ReplyTo { get; set; }
    
    public IDictionary<string, string> DataContext { get; set; }
    
    public IDictionary<string, string> Subject { get; set; }

    public bool UseOwnEmail { get; set; }
    
    public bool UseHtmlInDataContext { get; set; }

    public bool RaiseEvent { get; set; }
    
    public string CorrelationId { get; set; }
    
    public bool OfflineNotification { get; set; }
    
    public bool UseIntegrationService { get; set; }
    
    public SendMailModel()
    {
        Subject = new Dictionary<string, string>();
        DataContext = new Dictionary<string, string>();
        To = new List<string>();
        Cc = new List<string>();
        Bcc = new List<string>();
        Attachments = new List<string>();
        ReplyTo = new List<string>();
    }
}