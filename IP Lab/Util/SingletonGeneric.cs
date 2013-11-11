//*******************************************************************
//                                                                  *
//                          单件模式                          *
//                                                                  *
//*******************************************************************
namespace IP_Lab.Util
{
    /// <summary>
    /// 泛型单件模式
    /// </summary>
    public class SingletonGeneric<T> where T : new()
    {
        private static T _instance = default(T);
        private static object _syncRoot = new object();

        private SingletonGeneric()
        {

        }

        public static T GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
