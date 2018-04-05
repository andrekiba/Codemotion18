using System;
using System.Diagnostics;

namespace Xamrealm.Base
{
    public static class AsyncErrorHandler
    {
        public static void HandleException(Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }
}
