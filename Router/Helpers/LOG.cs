using System;

namespace Router.Helpers
{
    class LOG
    {
        private JSONArray Array = new JSONArray();

        public void Write(LOGType LOGType, params object[] Objects)
        {
            var obj = new JSONObject();
            obj.Push("type", LOGType);
            obj.Push("datetime", DateTime.Now);

            var arr = new JSONArray();
            foreach (var Object in Objects)
            {
                arr.Push(Object);
            }
            obj.Push("data", arr);

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

        public JSON Export()
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
