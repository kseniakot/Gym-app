using Microsoft.Extensions.Logging;
using Gym.Services;
using Gym.ViewModel;
using Gym.View;
using Gym.Model;
using Syncfusion.Maui.Core.Hosting;

namespace Gym
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
         //   builder.Services.AddSingleton<DBContext>();
          //  builder.Services.AddSingleton<DataBaseService>();

           
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<TokenService>();

            builder.Services.AddSingleton<WebService>();

            builder.Services.AddTransient<UserShellViewModel>();
            builder.Services.AddTransient<UserShell>();

            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<SignUpView>();

            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignInView>();

            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ForgotPasswordView>();

            builder.Services.AddTransient<UserListViewModel>();
            builder.Services.AddTransient<UserListView>();

            builder.Services.AddTransient<MembershipListViewModel>();
            builder.Services.AddTransient<MembershipListView>();

            builder.Services.AddTransient<FreezeListViewModel>();
            builder.Services.AddTransient<FreezeListView>();

            builder.Services.AddTransient<AddUserViewModel>();
            builder.Services.AddTransient<AddUserView>();

            builder.Services.AddTransient<AddMembershipViewModel>();
            builder.Services.AddTransient<AddMembershipView>();

            builder.Services.AddTransient<AddFreezeViewModel>();
            builder.Services.AddTransient<AddFreezeView>();

            builder.Services.AddTransient<EditMembershipViewModel>();
            builder.Services.AddTransient<EditMembershipView>();

            builder.Services.AddTransient<EditFreezeViewModel>();
            builder.Services.AddTransient<EditFreezeView>();

            builder.Services.AddTransient<BannedListViewModel>();
            builder.Services.AddTransient<BannedListView>();

            builder.Services.AddTransient<ShopViewModel>();
            builder.Services.AddTransient<ShopView>();

            builder.Services.AddTransient<MembershipViewModel>();
            builder.Services.AddTransient<MembershipView>();

            builder.Services.AddTransient<BuyMembershipViewModel>();
            builder.Services.AddTransient<BuyMembershipView>();

            builder.Services.AddTransient<UserMainViewModel>();
            builder.Services.AddTransient<UserMainPage>();

            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<ProfileView>();

            builder.Services.AddTransient<ActivateMembershipViewModel>();
            builder.Services.AddTransient<ActivateMembershipView>();

            builder.Services.AddTransient<FreezeMembershipViewModel>();
            builder.Services.AddTransient<FreezeMembershipView>();

            builder.Services.AddTransient<CancelFreezeViewModel>();
            builder.Services.AddTransient<CancelFreezeView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
