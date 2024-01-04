

namespace Application.Common.Models
{
    public class SequenceNumberQueryResponse
    {
        public string Context { get; set; }
        public long CurrentNumber { get; set; }
        public string[] Errors { get; set; }
    }
}
