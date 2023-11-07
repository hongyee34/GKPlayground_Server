using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKPlayground
{
    public class Singleton<T> where T : Singleton<T>, new() //제너릭 형식 제약조건(옵션)
    {
        private static Lazy<T> lazyInstance = null;

        public static T Instance
        {
            get
            {
                if (Exists() == false)
                {
                    var instance = new T();
                    lazyInstance = new Lazy<T>(() => instance);
                }

                return lazyInstance.Value;
            }
        }
        //인스턴스가 만들어졌는지 체크합니다.
        public static bool Exists()
        {
            return lazyInstance != null && lazyInstance.IsValueCreated;
        }
        //인스턴스 생성이력을 초기화 할때 사용합니다.
        public static void ClearInstance()
        {
            lazyInstance = null;
        }
    }
}
