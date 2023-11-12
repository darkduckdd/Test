// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("lxQaFSWXFB8XlxQUFZHPR0mrjVkllxQ3JRgTHD+TXZPiGBQUFBAVFuG8m0lXOuh0yoR3H7Q8MvIDMZQdwuov0XdUsOf4HfU7IfIJq9kR/QD6HkLpMnGhzTrrMsJ0pJEZ9cIlX7+ZV/pn9314qsXJ5Add3WKF4Rbf96IAYPJ+TBoIHgKdX3pwFZu9VSMl8c16Rpzyy7JLKZGhsBqR0Eprs3tn1mHclCb2gpLMJze25Rb1gUgrj5avihoc09w8jN8MSkgx55VYhAoNVOt7VIsOEUxKfdn+Ldmt3qRt3zVtvOZBgPjL4r9ls+PsR3Uh4ap3+vgc39hQ/zxzzy/WE0Hj92zeThFV9s/3NN5zj/pm/dr3EYUUc3o0gwDyaevuBxy24BcWFBUU");
        private static int[] order = new int[] { 1,1,7,7,13,12,8,12,10,13,13,12,13,13,14 };
        private static int key = 21;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
