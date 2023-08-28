namespace ET.Server
{
    [ComponentOf(typeof(Session))]
    public class SessionActorComponent : Entity, IAwake, IDestroy
    {
        public long ActorId { get; set; }
    }
}