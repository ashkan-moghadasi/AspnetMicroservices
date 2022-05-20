using System;

namespace EventBus.Messages.Events
{
    public class IntegrationBaseEvent
    {
        public IntegrationBaseEvent(Guid id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        public IntegrationBaseEvent()
        {
            Id = Guid.NewGuid();
            CreationDate=DateTime.Now;
        }

        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        
    }
}