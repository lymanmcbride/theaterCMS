//Edit method
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Edit([Bind(Include = "RentalRequestID,ContactPerson,Company,RequestedTime,StartTime,EndTime," +
    "ProjectInfo,RentalCode,Accepted,ContractSigned")] RentalRequest rentalRequest, string[] selectedRentals /*List containing associated rentals selected by user*/)
{
    List<Rental> deleteRelationship = new List<Rental>(); // list of rentals that will no longer be associated with rental request
    rentalRequest.Rentals = new List<Rental>();

    // populate rentals list property of RentalRequest
    if (selectedRentals != null)
    {
        foreach (var rental in selectedRentals)
        {
            var rentalToAdd = db.Rentals.Find(int.Parse(rental));
            rentalRequest.Rentals.Add(rentalToAdd);
            rentalToAdd.RentalRequestID = rentalRequest.RentalRequestID;
        }
    }

    // populate deleteRelationship list, and modify Rentals db entry
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
    return View(rentalRequest);
}

// methods for populating lists on the view
private void PopulateNullRentalsList()
{
    var rentals = db.Rentals.ToList();
    List<Rental> nullRentals = new List<Rental>();
    foreach (Rental rental in rentals)
    {
        if (rental.RentalRequestID == null)
        {
            nullRentals.Add(rental);
        }
    }
    ViewBag.nullRentals = nullRentals; //viewbag is a container for passing objects to the view
}

private void PopulateAssociatedRentalsList(int key)
{
    var rentals = db.Rentals.ToList();
    List<Rental> associatedRentals = new List<Rental>();
    foreach (Rental rental in rentals)
    {
        if (rental.RentalRequestID == key)
        {
            associatedRentals.Add(rental);
        }
    }
    
    ViewBag.associatedRentals = associatedRentals;
}