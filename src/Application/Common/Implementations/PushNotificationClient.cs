using Application.Common.Abstractions;
using Application.Common.Enums;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Selise.Ecap.Infrastructure;

namespace Application.Common.Implementations;

public class PushNotificationClient : IPushNotificationClient
    {
        private readonly Uri _pushNotificationUri;
        private readonly IRestCommunicationClient _restService;
        private readonly IRmwLogger<PushNotificationClient> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Base constructor 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="rmwSettings"></param>
        /// <param name="configuration"></param>
        /// <param name="restService"></param>
        public PushNotificationClient(
                IRmwLogger<PushNotificationClient> logger,
                IRmwSettings rmwSettings,
                IConfiguration configuration,
                IRestCommunicationClient restService)
        {
            _logger = logger;
            _configuration = configuration;
            _pushNotificationUri = DeterminePushNotificationEndPointUri(rmwSettings);
            _restService = restService;
        }

        /// <summary>
        /// Get Push Notification Endpoint
        /// </summary>
        /// <param name="rmwSettings"></param>
        /// <returns></returns>
        private Uri DeterminePushNotificationEndPointUri(IRmwSettings rmwSettings)
        {
            var notificationService = rmwSettings
                .EcapServices
                .Services
                .FindLast(x => x.ServiceName == EcapServiceType.Notification);

            if (notificationService is null)
            {
                return new Uri(string.Empty);
            }
            
            var pushNotificationEndPointUri = $"{notificationService.ServiceUrl}/{notificationService.ServiceVersion}/{notificationService.ServiceEndpoint["Notify"]}";
            
            _logger.LogDebug($"Push Notification URI: {pushNotificationEndPointUri}");
            
            return new Uri(pushNotificationEndPointUri);
        }

        /// <summary>
        /// Notify Client 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="context"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task NotifyToClientAsync<T>(T notification, string context, bool success, string errorMessage)
        {
            var notifierPayload = new
            {
                NotificationType = "FilterSpecificReceiverType",

                SubscriptionFilters = new[]
                {
                    new
                    {
                        ActionName = "", Context = context, Value = ""
                    }
                },

                ResponseKey = "RmwNotificationPayload",

                DenormalizedPayload = JsonConvert.SerializeObject(new
                {
                    Result = notification,
                    ErrorMessage = errorMessage,
                    Success = success
                }),
                ResponseValue = JsonConvert.SerializeObject(new { Success = success }),
            };

            return NotifyAsync(notifierPayload);
        }

        /// <summary>
        /// Notify specific user 
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="messageKey"></param>
        /// <param name="userIds"></param>
        /// <param name="success"></param>
        /// <param name="errorMessage"></param>
        /// <param name="detailRoute"></param>
        /// <param name="responseKey"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task NotifyToUserAsync<T>(
            T notification, 
            string messageKey,
            IEnumerable<string> userIds, 
            bool success, 
            string errorMessage,
            string detailRoute = null,
            string responseKey = null, IEnumerable<string> dynamicMessageKey = null)
        {
            var notifierPayload = new
            {
                NotificationType = "UserSpecificReceiverType",
                UserIds = userIds,
                DenormalizedPayload = JsonConvert.SerializeObject(new
                {
                    MessageKey = GetMessageKey(dynamicMessageKey, messageKey),
                    Result = notification,
                    ErrorMessage = errorMessage,
                    Success = success,
                    DetailRoute = detailRoute,
                }),
                ResponseValue = JsonConvert.SerializeObject(new { Success = success }),
                ResponseKey = responseKey,
            };

            return NotifyAsync(notifierPayload);
        }

        private static object GetMessageKey(IEnumerable<string> dynamicMessageKey, string messageKey)
        {
            if(dynamicMessageKey?.Count() > 0)
            {
                return dynamicMessageKey;
            }
            return messageKey;
        }

        /// <summary>
        /// Notify 
        /// </summary>
        /// <param name="payload"></param>
        public async Task NotifyAsync(object payload)
        {
            _logger.LogInformation($"Push Notification Uri: {_pushNotificationUri}");
            await _restService.PostRequestAsync(_pushNotificationUri, payload);
        }
    }