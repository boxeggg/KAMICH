using Android.Content;
using AndroidX.Core.Content;
using Uri = Android.Net.Uri;

namespace KAMICH.Platforms.Android;

public static class ApkInstaller
{
    public static void InstallApk(string filePath)
    {
        var context = global::Android.App.Application.Context;
        var file = new Java.IO.File(filePath);

        Uri apkUri = AndroidX.Core.Content.FileProvider.GetUriForFile(
            context,
            $"{context.PackageName}.fileprovider",
            file);

        var intent = new Intent(Intent.ActionView);
        intent.SetDataAndType(apkUri, "application/vnd.android.package-archive");
        intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.GrantReadUriPermission);

        context.StartActivity(intent);
    }
}