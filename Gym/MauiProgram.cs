using Microsoft.Extensions.Logging;
using Gym.Services;
using Gym.ViewModel;
using Gym.View;
using Gym.Model;

namespace Gym
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<DBContext>();
            builder.Services.AddSingleton<DataBaseService>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<SignUpView>();

            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignInView>();

            builder.Services.AddTransient<UserListViewModel>();
            builder.Services.AddTransient<UserListView>();

            builder.Services.AddTransient<MembershipListViewModel>();
            builder.Services.AddTransient<MembershipListView>();

            builder.Services.AddTransient<AddUserViewModel>();
            builder.Services.AddTransient<AddUserView>();

            builder.Services.AddTransient<AddMembershipViewModel>();
            builder.Services.AddTransient<AddMembershipView>();

            builder.Services.AddTransient<EditMembershipViewModel>();
            builder.Services.AddTransient<EditMembershipView>();

            builder.Services.AddTransient<BannedListViewModel>();
            builder.Services.AddTransient<BannedListView>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
