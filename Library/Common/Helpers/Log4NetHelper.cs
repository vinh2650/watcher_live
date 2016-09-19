using log4net;
using log4net.Config;

namespace Common.Helpers
{
    public  class Log4NetHelper
    {
        private static ILog _logger;

        public static ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    XmlConfigurator.Configure();
                    _logger = LogManager.GetLogger("AmsLog");

                }
                return _logger;
            }
        }


    }
}
