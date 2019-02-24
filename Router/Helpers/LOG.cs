using System;

namespace Router.Helpers
{
    class LOG
    {
        private old_JSONArray Array = new old_JSONArray();

        public void Write(LOGType LOGType, params object[] Objects)
        {
            var obj = new old_JSONObject();
            obj.Add("type", LOGType);
            obj.Add("datetime", DateTime.Now);

            var arr = new old_JSONArray();
            foreach (var Object in Objects)
            {
                arr.Push(Object);
            }
            obj.Add("data", arr);

            Array.Push(obj);
        }

        public void Push(params object[] Objects) => Write(LOGType.Log, Objects);

        public void Success(params object[] Objects) => Write(LOGType.Success, Objects);

        public void Info(params object[] Objects) => Write(LOGType.Info, Objects);

        public void Warning(params object[] Objects) => Write(LOGType.Warning, Objects);

        public void Danger(params object[] Objects) => Write(LOGType.Danger, Objects);

        public void FatalError(params object[] Objects) => Write(LOGType.FatalError, Objects);

        public void Empty()
        {
            Array.Empty();
        }

        public old_JSON Export()
        {
            return Array;
        } 
    }

    enum LOGType : int
    {
        Log = 0,
        Success = 1,
        Info = 2,
        Warning = 3,
        Danger = 4,
        FatalError = 5
    }
}
