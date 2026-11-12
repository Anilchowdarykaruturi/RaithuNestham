
using FirebaseAdmin.Messaging;

namespace RaithuNestham.Services;

public class FirebasePushNotificationService
{
	public async Task<string> SendToDeviceAsync(
		string fcmToken,
		string title,
		string body)
	{
		if (string.IsNullOrWhiteSpace(fcmToken))
		{
			throw new ArgumentException(
				"FCM token is required.",
				nameof(fcmToken));
		}

		var message = new Message
		{
			Token = fcmToken,

			Notification = new Notification
			{
				Title = title,
				Body = body
			},

			Data = new Dictionary<string, string>
			{
				{ "type", "weather_alert" }
			},

			Android = new AndroidConfig
			{
				Priority = Priority.High
			}
		};

		return await FirebaseMessaging.DefaultInstance
			.SendAsync(message);
	}
}
