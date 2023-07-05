﻿using System.Net;

namespace ET.Server
{
    [Invoke((long)SceneType.NetInner)]
    public class FiberInit_Net: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<ActorSenderComponent, SceneType>(SceneType.NetInner);
            root.AddComponent<ActorRecverComponent>();
            StartMachineConfig startMachineConfig = StartMachineConfigCategory.Instance.Get(fiberInit.Fiber.Process);
            IPEndPoint ipEndPoint = NetworkHelper.ToIPEndPoint($"{startMachineConfig.InnerIP}:{startMachineConfig.WatcherPort}");
            root.AddComponent<NetInnerComponent, IPEndPoint>(ipEndPoint);

            await ETTask.CompletedTask;
        }
    }
}