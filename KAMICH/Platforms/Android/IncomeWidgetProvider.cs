using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace KAMICH;

[BroadcastReceiver(Label = "Przychód KAMICH", Exported = true)]
[IntentFilter(new[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
[MetaData("android.appwidget.provider", Resource = "@xml/widget_income_info")]
public class IncomeWidgetProvider : AppWidgetProvider
{
    public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
    {
        if (context == null || appWidgetManager == null || appWidgetIds == null) return;

        foreach (var widgetId in appWidgetIds)
        {
            UpdateWidget(context, appWidgetManager, widgetId);
        }
    }

    public static void UpdateWidget(Context context, AppWidgetManager appWidgetManager, int widgetId)
    {
        var views = new RemoteViews(context.PackageName, Resource.Layout.widget_income);

        // Read income from shared preferences
        var prefs = context.GetSharedPreferences("kamich_widget", FileCreationMode.Private);
        var income = prefs?.GetFloat("daily_income", 0f) ?? 0f;

        views.SetTextViewText(Resource.Id.widget_income, $"{income:N2} zł");

        // Click opens the app
        var intent = new Intent(context, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
        var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
        views.SetOnClickPendingIntent(Resource.Id.widget_income, pendingIntent);

        appWidgetManager.UpdateAppWidget(widgetId, views);
    }
}
