using Android.Appwidget;
using Android.Content;

namespace KAMICH;

public static class WidgetHelper
{
    public static void UpdateWidgetData(double income, string label)
    {
        var context = Android.App.Application.Context;

        // Save to shared preferences so widget and worker can read it
        var prefs = context.GetSharedPreferences("kamich_widget", Android.Content.FileCreationMode.Private);
        var editor = prefs?.Edit();
        editor?.PutFloat("daily_income", (float)income);
        editor?.PutString("income_label", label);
        editor?.Apply();

        // Notify widget to refresh
        RefreshAllWidgets(context);
    }

    /// <summary>
    /// Save API key to widget shared prefs so the background Worker can access it.
    /// SecureStorage is encrypted and not accessible from Worker context.
    /// </summary>
    public static void SaveApiKeyForWorker(string? apiKey)
    {
        var context = Android.App.Application.Context;
        var prefs = context.GetSharedPreferences("kamich_widget", Android.Content.FileCreationMode.Private);
        var editor = prefs?.Edit();
        if (string.IsNullOrEmpty(apiKey))
            editor?.Remove("api_key");
        else
            editor?.PutString("api_key", apiKey);
        editor?.Apply();
    }

    private static void RefreshAllWidgets(Context context)
    {
        var appWidgetManager = AppWidgetManager.GetInstance(context);
        var widgetComponent = new ComponentName(context, Java.Lang.Class.FromType(typeof(IncomeWidgetProvider)));
        var widgetIds = appWidgetManager?.GetAppWidgetIds(widgetComponent);

        if (widgetIds != null && widgetIds.Length > 0)
        {
            foreach (var id in widgetIds)
            {
                IncomeWidgetProvider.UpdateWidget(context, appWidgetManager!, id);
            }
        }
    }
}
