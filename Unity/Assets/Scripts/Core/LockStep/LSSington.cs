using System;
using System.Collections.Generic;

namespace ET
{
    [UniqueId(-1, 1)]
    public static class LSQueneUpdateIndex
    {
        public const int None = -1;
        public const int LSUpdate = 0;
        public const int Max = 1;
    }
    
    public class LSSington: Singleton<LSSington>, ISingletonAwake
    {
        private readonly TypeSystems typeSystems = new(LSQueneUpdateIndex.Max);
        
        public void Awake()
        {
            foreach (Type type in EventSystem.Instance.GetTypes(typeof (LSSystemAttribute)))
            {
                object obj = Activator.CreateInstance(type);

                if (obj is not ISystemType iSystemType)
                {
                    continue;
                }

                TypeSystems.OneTypeSystems oneTypeSystems = this.typeSystems.GetOrCreateOneTypeSystems(iSystemType.Type());
                oneTypeSystems.Map.Add(iSystemType.SystemType(), obj);
                int index = iSystemType.GetInstanceQueueIndex();
                if (index > InstanceQueueIndex.None && index < InstanceQueueIndex.Max)
                {
                    oneTypeSystems.QueueFlag[index] = true;
                }
            }
        }
        
        public TypeSystems.OneTypeSystems GetOneTypeSystems(Type type)
        {
            return this.typeSystems.GetOneTypeSystems(type);
        }
        
        public void Rollback(Entity entity)
        {
            if (entity is not IRollback)
            {
                return;
            }
            
            List<object> iRollbackSystems = this.typeSystems.GetSystems(entity.GetType(), typeof (IRollbackSystem));
            if (iRollbackSystems == null)
            {
                return;
            }

            foreach (IRollbackSystem iRollbackSystem in iRollbackSystems)
            {
                if (iRollbackSystem == null)
                {
                    continue;
                }

                try
                {
                    iRollbackSystem.Run(entity);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void LSUpdate(LSEntity entity)
        {
            if (entity is not ILSUpdate)
            {
                return;
            }
            
            List<object> iLSUpdateSystems = typeSystems.GetSystems(entity.GetType(), typeof (ILSUpdateSystem));
            if (iLSUpdateSystems == null)
            {
                return;
            }

            foreach (ILSUpdateSystem iLSUpdateSystem in iLSUpdateSystems)
            {
                try
                {
                    iLSUpdateSystem.Run(entity);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}