using MeterDataDashboard.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MeterDataDashboard.Infra.Identity
{
    public class AppIdentityInitializer : IAppIdentityInitializer
    {
        public UserManager<ApplicationUser> UserManager { get; set; }
        public RoleManager<IdentityRole> RoleManager { get; set; }
        public IConfiguration Configuration { get; set; }

        /**
         * This method seeds admin, guest role and admin user
         * **/
        public async void SeedData()
        {
            // get admin params from configuration
            UserInitVariables initVariables = new UserInitVariables();
            initVariables.InitializeFromConfig(Configuration);
            // seed roles
            await SeedUserRoles(RoleManager);
            // seed admin user
            await SeedUsers(UserManager, initVariables);
        }

        /**
         * This method seeds admin and guest users
         * **/
        public async Task SeedUsers(UserManager<ApplicationUser> userManager, UserInitVariables initVariables)
        {
            await SeedUser(userManager, initVariables.AdminUserName, initVariables.AdminEmail,
                initVariables.AdminPassword, SecurityConstants.AdminRoleString);
            await SeedUser(userManager, initVariables.GuestUserName, initVariables.GuestEmail,
                initVariables.GuestPassword, SecurityConstants.GuestRoleString);
        }

        /**
         * This method seeds a user
         * **/
        public async Task SeedUser(UserManager<ApplicationUser> userManager, string userName, string email, string password, string role)
        {
            // check if user doesn't exist
            if ((await userManager.FindByNameAsync(userName)) == null)
            {
                // create desired user object
                ApplicationUser user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email
                };

                // push desired user object to DB
                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        /**
         * This method seeds roles
         * **/
        public async Task SeedUserRoles(RoleManager<IdentityRole> roleManager)
        {
            await SeedRole(roleManager, SecurityConstants.GuestRoleString);
            await SeedRole(roleManager, SecurityConstants.AdminRoleString);
        }

        /**
         * This method seeds a role
         * **/
        public async Task SeedRole(RoleManager<IdentityRole> roleManager, string roleString)
        {
            // check if role doesn't exist
            if (!(await roleManager.RoleExistsAsync(roleString)))
            {
                // create desired role object
                IdentityRole role = new IdentityRole
                {
                    Name = roleString,
                };
                // push desired role object to DB
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
