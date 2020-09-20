using log4net;
using log4net.Config;

[assembly: XmlConfigurator]

namespace BeerRater.Utils
{
    /// <summary>The log util.</summary>
    public static class LogUtil
    {
        /// <summary>Initializes static members of the <see cref="LogUtil" /> class.</summary>
        static LogUtil()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>The get logger.</summary>
        /// <returns>The <see cref="ILog" />.</returns>
        public static ILogger GetLogger()
        {
            return GetLogger(AppUtil.EntryModuleName);
        }

        /// <summary>
        ///     The get logger.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <returns>
        ///     The <see cref="ILog" />.
        /// </returns>
        public static ILogger GetLogger(string name)
        {
            return new Log4NetWrapper(LogManager.GetLogger(name));
        }
    }
}