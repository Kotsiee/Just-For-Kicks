// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("K8B4NwYOB4hzGwRM2LbAQ4YAvQy1RFjIo/jU/0/TKtlICocKbewWu5CL7loXxI4JM/PS9lMJ7psP/2onEOcmOzKT1W8Q8FxXHTUkSqMcXGZQ4mFCUG1maUrmKOaXbWFhYWVgYzx+16rykOREIFRYYvoD/zatHUB3dLQy9iAMD6evxlEgWK5boR3wi0VeHDpffaCbSRJX76z+/sh7TYeWiiUGh92TUxDP/+a482FcOUf169cjBrpf0m0HOIuEpAqpV1k/5NV8KdLiYW9gUOJhamLiYWFgu+Dh3tf8l8SnOh90Rx6Kyk+HyhAQEdzAEiByfZS2JwMf+yTPzHkkr6P2SUDp1mtO4b5FA7KJr8fgv0tTlDldoilKgMSDsU3S5C3Jm2JjYWBh");
        private static int[] order = new int[] { 9,3,4,8,9,10,13,12,11,13,11,11,13,13,14 };
        private static int key = 96;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
