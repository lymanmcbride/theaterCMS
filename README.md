# Theater Vertigo CMS Project
4/19-30/2021
## Introduction
The Theater Vertigo CMS Project was a two week sprint where my team was tasked with building out a section of a new website for a theater in Portland. The project had been ongoing, but for our section, we had the opportunity to build out CRUD functionality, set up permissions and roles, and style the front end. 

Over the two week period we utilized Agile/Scrum methodologies for team and project management, git version control, and AzureDevOps as our PM medium. 

The full stack stories comprise the most challenging sections of the project, especially the first two, and involved code in multiple languages working together to present the user with a clean experience. 

## Contents
### The Stories
- [Making the Models](##making-the-models)
- [Create Rental Manager](###create-rental-manager)
- [Link the Rentals models to Requests models](###link-rentals-to-rental-requests)
- [Index Filter Feature](###index-filter)

### Notes
- [Styling](##a-note-on-styling)
- [Website Pictures Consolidated](#front-end-pictures-consolidated)

## Making the Models
[Jump to Front End Portion](###models-front-end)
### Models Back End
My assigned section of the site was handling rentals for the theater. There were three types of rentals the company could potentailly be dealing with: a general rental, rooms, and equipment.
#### Create the Models
The C# models I created needed to handle these criteria. They would all have properties from a general rental category, but rooms and equipment had addidional properties that were different from each other. Because of this I implemented inheritance in the structure: 
![Model Structure](/img/story1-models-2.jpg)

After making the models, I scaffolded the CRUD pages using Entity Framework, which provided basic CRUD functionality for the parent class (rental). 
### Build out CRUD functionality for Inherited Classes
![CRUD three models](/img/story2-CreateAndEditPages.JPG)
This story turned out to be one of the trickiest on the project. Because of the inheritance implementation, these models had the ability to perform CRUD operations using the parent model, however the front end pages simply didn't have access to them. This story evolved into three parts to complete: 1. Add a view model, 2. change the back end logic to transfer information between the view model and the back end models, and 3. use JavaScript to display the form correctly on each page. 

1. The database stored the information for the models all in the same table, however it used a delimiter to assign the rental type for each entry when retrieved. Only one model can be used on a view, so in order to handle this I created a view model that contained all properties for all three types of rentals. This helped display the models on the page, but the next issue was how to get the models from the database into the view model. To accomplish this, I added an overloaded constructor to assist with the back end logic. The main constructor takes a rental object, and assignes properties to it based on whether the rental is of the parent or child types. The second constructor is empty and takes no parameter, allowing for an instantiation of the view model object without assigning properties. Below are my two constructors.
```csharp
public class AllRentals
    {
        //Constructor needed to map each rental type to the view model.
        public AllRentals(Rental rental)
        {
            RentalId = rental.RentalId;
            RentalName = rental.RentalName;
            RentalCost = rental.RentalCost;
            FlawsAndDamages = rental.FlawsAndDamages;
            RentalType = "rental";
            RentalRequestID = rental.RentalRequestID;
            if (rental is RentalEquipment equipment) //"is" statement makes sure it's the right type
            {
                ChokingHazard = equipment.ChokingHazard;
                SuffocationHazard = equipment.SuffocationHazard;
                PurchasePrice = equipment.PurchasePrice;
                RentalType = "equipment";
            }
            else if (rental is RentalRoom room)
            {
                RoomNumber = room.RoomNumber;
                SquareFootage = room.SquareFootage;
                MaxOccupancy = room.MaxOccupancy;
                RentalType = "room";
            }
        }
        //empty overloaded constructor to edit and create
        public AllRentals() { }
```

2. The Controller Methods were made simple by the above logic, allowing for an easy instantiation of the view model, passing in the rental retrieved from the database as a parameter. Below is the basic logic (I've stripped out the conditionals controlling for errors). The constructor will assign properties based on whether the rental in the database is general, room, or equipment.
```csharp
public ActionResult Details(int? id)
        {
            var rental = db.Rentals.Find(id);
            AllRentals allRentals = new AllRentals(rental);
            return View(allRentals);
```

### Models Front End
3. The view required some creative thinking as well. I needed the ability to hide/show form fields based on the user's choice for the rental type. I wrote JavaScript functions to accomplish this. I also needed a way to tell the controller method what type of rental would be coming in. I added a hidden form field to carry this information to the controller. The JavaScript shows and hides form fields and also changes the value of the rental type form field based on the user's choice of rental.
```javascript
// Handles changing form to different types on create, edit, and details pages
function rentalChange(value) {
    //all potential elements to hide
    var hideElements = document.getElementsByClassName("to-hide"); 

    //reset all to 'hidden'
    for (i = 0; i < hideElements.length; i++) {
        var listOfClasses = hideElements[i].classList;
        if (!listOfClasses.contains("d-none")) {
            listOfClasses.add("d-none");
        }
    }

    //show only elements that are needed for the model
    var hiddenElements = document.getElementsByClassName(value);
    for (i = 0; i < hiddenElements.length; i++) {
        hiddenElements[i].classList.remove("d-none");
    }

    //changes value on last element (hidden)
    document.getElementById("rentalType").value = value
}
```
This code allowed the user to change the rental type, however the edit page only needed to run this function when the page loads. The timing of running the script in this framework necessitated calling this function in the .js file.  
```javascript
if (document.URL.includes("/Rent/Rentals/Edit/")
    || document.URL.includes("/Rent/Rentals/Create")) {
    const dataElement = document.querySelector(".form-horizontal");
    if (dataElement.dataset.type != "") {
        window.onload = rentalChange(dataElement.dataset.type);
    }
}
```
The final look provided three different forms based on the rental type and ability to CRUD each of them:

![Rental Form](/img/Rental.JPG)
![Equipment Form](/img/Equipment.JPG)
![Room Form](/img/Rooms.JPG)

Jump to: 
- [Top](#theater-vertigo-cms-project)
- [Back End portion of this story](###models-back-end)

## Create Rental Manager

The next several stories comprised the creation of the Rental Manager user/role. I created the user, then I was then tasked with creating a button that could easily log in the rental manager for development purposes, and finally I restricted crud operations to the user in that role. 

### Rental Manager Back End
[Jump to Front End Portion](###rental-manager-front-end)

The Rental Manager class extends from ApplicationUser, and only adds two properties. Once I created the class, I then seeded a rental manager user into the database for development purposes, using a static seed method within the class. The static method allowed me to call this in the global seed method (configuration.cs file). For the whole class see [RentalManager.cs](/RentalManager.cs) 

I explain below how I hid the button access to CRUD pages, however in case a user tried to bypass the buttons, and typed in the url to view a forbidden page, access was restricted on the back end using the AuthorizeAttribute Data Annotation class. I overrode two methods in order to create the class which checked for the user role of "rental manager" and redirected to an "access denied" page if not found. 
```csharp
// authorization class for RentalManager role. 
    public class RentalManagerAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)//checks for rental manager role
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                if (httpContext.User.IsInRole("RentalManager")) 
                {
                    return true;
                }
            }
            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) //handles redirect to access denied page
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Rentals/AccessDenied" }));
        }
    }
```
The above code allowed me to use this data annotation in the controller, which I placed above the create, details, and delete methods: 
```csharp
[RentalManagerAuthorize] //Checks that user has Rental Manager Role
public ActionResult Create()
{
    return View();
}
```

### Rental Manager Front End

The next requirement was to create an easy login button for development purposes. By clicking on the button, the developer would automatically be logged in as the manager. It took me a while to realize that I could create a form where all the fields were hidden except the submit button. All information necessary for Login is contained in the form, and once clicked would be passed to the LoginViewModel. The button only shows up if the user is not logged in, and is only present in the rentals area, a problem which I solved by placing the code for the button in a partial view, and then only displaying the partial view if the url contained /rent/rentals.
![Rental Manager Login](/img/loginbutton.JPG)

Restricting CRUD access on the front end was just a matter of getting the user role and, if it was "RentalManager", hiding the buttons to access the CRUD pages.
![Rental CRUD access buttons](/img/card-footer.JPG)



Jump to: 
- [Top](#theater-vertigo-cms-project)
- [Jump to Back End Portion of this story](###rental-manager-back-end)
- [Index Filter](###index-filter)

## Link Rentals to Rental Requests
![Link Rentals to Requests](/img/story13-1.JPG)

Near the end of the project, my colleage had completed the section dealing with Rental Requests. I was then tasked with linking the Rental models to the Rental Requests models. This story included quite a bit of full stack development, which I split into 3 parts: create the relationship in the models, implement relationship CRUD functionality on Rental Requests, and show the relationship on the Rentals index page.

1. One to many relationships on ASP.NET are created through a foreign key property on the "is-a" model (is a rental) and a list property on the "has-a" model (has a rental: meaning the request). 
```csharp
//add to the requests model:
//One to Many relationship with Rentals. This is the list of rentals assigned to each request. 
public ICollection<Rental> Rentals { get; set; }

//add to the rentals model:
//Relationship to Requests
[ForeignKey("RentalRequest")]
public int? RentalRequestID { get; set; }
public RentalRequest RentalRequest { get; set; }
```

2. The CRUD functionality on Rental Requests required a lot of code both on the front end and the back end. Starting with the front end, I created a select field on the Create-Request page which populated based on rentals in the database which didn't already have a relationship assigned. The details page was even more complicated as it also displayed rentals that were already assigned to the request and selected them by default. By naming the field, I made sure that whatever was selected by the user would be passed back to the controller as an argument.
![CRUD Detail Page](/img/razor_selectedRentals.JPG)

Once the user selects rentals, they are passed to the controller as a string list. I then wrote logic in the controller that populates the rentals list property of the request, deletes the foreign key from rentals that are no longer associated with the request, and adds foreign keys to new rentals that are now associated with the rental request. I also wrote methods that populate lists of rentals based on their foreign key property in order to pass them to the view. Below is the logic for saving these properties, [view full methods described](/RentalRequestsController.cs)
```csharp
if (selectedRentals != null)
    {
        foreach (var rental in selectedRentals)
        {
            var rentalToAdd = db.Rentals.Find(int.Parse(rental));
            rentalRequest.Rentals.Add(rentalToAdd);
            rentalToAdd.RentalRequestID = rentalRequest.RentalRequestID;
        }
    }

foreach (var rental in db.Rentals.ToList())
    {
        // if a rental is found that has foreign key of request, but is not in current list of associated rentals, delete the foreign key
        if ((rental.RentalRequestID == rentalRequest.RentalRequestID && !rentalRequest.Rentals.Contains(rental)))
        {
            rental.RentalRequestID = null;
            deleteRelationship.Add(rental);
        }
    }

if (ModelState.IsValid)
    {
        // save all modifications
        db.Entry(rentalRequest).State = EntityState.Modified;
        foreach (var rental in rentalRequest.Rentals)
        {
            db.Entry(rental).State = EntityState.Modified;
        }
        foreach (var rental in deleteRelationship)
        {
            db.Entry(rental).State = EntityState.Modified;
        }
        db.SaveChanges();
        return RedirectToAction("Index");
    }
```
3. The story required some modifications to the display on the Rentals pages as well. I added Razor logic so that if the rental's request property wasn't null (meaning it is associated with a request), the opacity was reduced. I also added a section to the details page that showed the associated request's information if it existed. The end result looked like this:
![Rental Index](/img/rentalIndex.JPG)
![Rental Details](/img/rental_details.JPG)

The razor/html code for the details page is as follows:
![Rental Details Code](/img/requestDetailsCode.JPG)

Jump to: 
- [Top](#theater-vertigo-cms-project)
- [Create Rental Manager](##create-rental-manager)

## Index Filter
![Index Filter](/img/story-10_indexFilter.JPG)
The Index page  filter evolved into a two part story involving the index controller method and javascript. I was also tasked with creating a search bar that populated as the user typed. 

### Back End Index Filter

1. First, I needed to use the view model again, so I could access the different types of rentals, but this time in an IEnumerable list, since I was accessing all entries and displaying them on the page. The filter box passes as an argument into the index method, allowing for a lambda function in a .Where clause to filter the results in the IEnumerable being passed to the view. This story also provided an opportunity to use some exception handling. All filtering was performed on the back end.
```csharp
if (!String.IsNullOrEmpty(searchCost))
    {
        try
        {
            decimal cost = Convert.ToDecimal(searchCost);
            if (greaterLessThan == "less")
            {
                iAllRentals = allRentals.Where(s => s.RentalCost < cost);
            }
            else
            {
                iAllRentals = allRentals.Where(s => s.RentalCost > cost);
                ViewBag.LessThanGreaterThan = "greater";
            }
        }
        catch (FormatException) //if letters are inputted
        {
            ViewBag.searchCostException = "Please enter numbers only";
        }
        catch (Exception) //catches any other exception
        {
            ViewBag.searchCostException = "Something went wrong. Please try again or contact site administrator.";
        }
```
For information, ViewBag is a container for elements being passed to the view. Because I filtered on the back end, I was also able to validate on the back end and return error messages from the server. 

### Front End index Filter

2. This form required quite a bit of JavaScript as well, as the user could click the "less than" or "greater than" button in order to switch how the information was being filtered. The JavaScript code switches the chevron symbol on each click, and changes the hidden value in the form (the value used to signal how to filter the rental objects). I also had to write a function that would be called on page load so that when the controller method returns the view with the filtered list, the chevron and value would be still be the way the user left it (meaning after they click filter, the chevron and value would not automatically switch back to default value). [See these functions in full](/filterjs.js)

### Index Search
![Name Search](/img/story-11_indexSearch.jpg)

In order to make a search feature work as the user was typing, I decided to use javascript (jQuery syntax) to show and hide rentals based on the name search criterea. For this search bar, all of the rentals are loaded, but are shown or hidden by the javascript. 
JQuery was very useful as this function selected a lot of elements from the page. First, it retrieved the information from the input element, and then for each bootstrap card, if input could be found in the title it would show or hide the card. 
```javascript
function searchCards() {
    var input = $("#rental_name_search").val().toLowerCase();
    $(".card").each(function () {
        var title = $(this).find(".card-title").text();
        if (title.toLowerCase().includes(input)) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}
```

Jump to: 
- [Top](#theater-vertigo-cms-project)
- [Create Rental Manager](##create-rental-manager)
- [Link Rental Models to Rental Request Models](##link-rentals-to-rental-requests)



## A note on styling
Our project did not use a front end framework, so all styling was accomplished by using HTML/CSS. The color palette was pre-defined with variable names, but much of the styling was then left up to the engineer. I've included several photos of each of the pages in various sections of this document. I'll provde them at the end as well.

## Project Management Skills
Our use of git version control, Azure DevOps, Agile and Scrum Methodologies made me much more confident in my ability to contribute to a team atmosphere in an efffective and efficient way. I thoroughly enjoyed working in a group, communicating needs in the daily standup, and seeking to make my communications as clear as possible through slack messages, email, video calls, commit messages, and pull request descriptions. I learned many important soft skills along the way. 

# Front End Pictures Consolidated
## Rental Index Page
![Rental Index](/img/rentalIndex.jpg)
It bears mentioning that the pill-badges "Choking Hazard" and "Suffocation Hazard" only display if the model's corresponding property is set to True. 
![Rental Index2](/img/index2.jpg)
## Rental Details Page
![Rental Details](/img/rental_details.jpg)
## Rental Forms
![Rental Form](/img/rental.JPG)
![Equipment Form](/img/equipment.JPG)
![Room Form](/img/rooms.JPG)