public class RentalManager : ApplicationUser
{
    public int RetiredRentals { get; set; }
    public int RefurbishedRentals { get; set; }


    public static void Seed(ApplicationDbContext db)
    {
        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

        if (!roleManager.RoleExists("RentalManager"))
        {
            //Create Role
            var role = new IdentityRole()
            {
                Name = "RentalManager"
            };
            roleManager.Create(role);

            //Create User
            var rentalManager = new RentalManager()
            {
                UserName = "rentalmanager",
                Email = "rmanager@rental.com",
                RetiredRentals = 30,
                RefurbishedRentals = 100
            };
            string rmanagerPWD = "R3nt@lM@n@g3r";
            var chkUser = userManager.Create(rentalManager, rmanagerPWD);

            if (chkUser.Succeeded)
            {
                userManager.AddToRole(rentalManager.Id, "RentalManager");
            }
        }
    }
}