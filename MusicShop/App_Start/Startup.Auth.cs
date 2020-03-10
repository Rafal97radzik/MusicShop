using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using MusicShop.App_Start;
using MusicShop.DAL;
using MusicShop.Models;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicShop
{
    public partial class Startup
    {
        // Aby uzyskać więcej informacji o konfigurowaniu uwierzytelniania, odwiedź stronę https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Skonfiguruj kontekst bazy danych, menedżera użytkowników i menedżera logowania, aby używać jednego wystąpienia na żądanie
            app.CreatePerOwinContext(StoreContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Zezwalaj aplikacji na przechowywanie w pliku cookie informacji o zalogowanym użytkowniku
            // oraz na tymczasowe przechowywanie w pliku cookie informacji o użytkowniku logującym się przy użyciu dostawcy logowania innego producenta
            // Konfiguruj plik cookie logowania
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Umożliwia aplikacji weryfikowanie znacznika zabezpieczeń podczas logowania się użytkownika.
                    // Jest to funkcja zabezpieczeń używana w przypadku zmiany hasła lub dodawania logowania zewnętrznego do konta.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Umożliwia aplikacji tymczasowe przechowywanie informacji o użytkownikach, gdy używają drugiego etapu w procesie uwierzytelniania dwuetapowego.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Umożliwia aplikacji zapamiętanie drugiego etapu uwierzytelniania logowania, takiego jak numer telefonu lub adres e-mail.
            // Jeśli zaznaczysz tę opcję, drugi etap weryfikacji w procesie logowania zostanie zapamiętany na urządzeniu, na którym się zalogowano.
            // Ta opcja działa podobnie do opcji RememberMe podczas logowania.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Usuń znaczniki komentarza z poniższych wierszy, aby włączyć logowanie przy użyciu innych dostawców logowania
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            var options = new FacebookAuthenticationOptions()
            {
                AppId = "1277993205729156",
                AppSecret = "0318d04bab75b56c38e036b792ebc3a9"
            };
            options.Scope.Add("email");

            app.UseFacebookAuthentication(options);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "175907720860-p4dbv917hmuf1kl7vlt11j0cv1vt8p4h.apps.googleusercontent.com",
                ClientSecret = "YtrvDzyo_HYjN53tW9OwkBWd"
            });
        }
    }
}