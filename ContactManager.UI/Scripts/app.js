const EmailAddressTypesEnum = Object.freeze({
    PersonalType: { name: "Personal", value: 1 },
    BusinessType: { name: "Business", value: 2 }
});

let error = ko.observableArray();
let Contact = function (id, firstName, lastName, emailId, emailAddress, emailAddressType) {
    const self = this;
    self.Id = ko.observable(ko.utils.unwrapObservable(id));
    self.FirstName = ko.observable(ko.utils.unwrapObservable(firstName));
    self.LastName = ko.observable(ko.utils.unwrapObservable(lastName));
    self.EmailId = ko.observable(ko.utils.unwrapObservable(emailId));
    self.EmailAddress = ko.observable(ko.utils.unwrapObservable(emailAddress));
    self.EmailAddressType = emailAddressType == null ? 1 : ko.observable(ko.utils.unwrapObservable(emailAddressType));
};

let ViewModel = function () {
    const self = this;

    self.Contacts = ko.observableArray();
    self.CSVContacts = ko.observableArray(); 
    self.detail = ko.observable();
    self.query = ko.observable('');

    self.filteredContacts = ko.computed(function () {
        const filter = self.query().toLowerCase();

        if (!filter) {
            return self.Contacts();
        } else {
            return ko.utils.arrayFilter(self.Contacts(), function (contact) {
                return contact.FirstName.toLowerCase().indexOf(filter) !== -1
                    || contact.LastName.toLowerCase().indexOf(filter) !== -1
                    || contact.EmailAddress.toLowerCase().indexOf(filter) !== -1
                    || (contact.EmailAddressType === EmailAddressTypesEnum.PersonalType.value && EmailAddressTypesEnum.PersonalType.name.toLowerCase().indexOf(filter) !== -1)
                    || (contact.EmailAddressType === EmailAddressTypesEnum.BusinessType.value && EmailAddressTypesEnum.BusinessType.name.toLowerCase().indexOf(filter) !== -1);
            });
        }
    }, self);

    const contactsUri = '/api/Contact/GetContacts';

    self.ValidateContact = function (contact) {
        if (!contact) {
            return false;
        }

        const currentContact = ko.utils.unwrapObservable(contact);
        const currentFirstName = ko.utils.unwrapObservable(currentContact.FirstName);
        const currentLastName = ko.utils.unwrapObservable(currentContact.FirstName);
        const currentEmailAddress = ko.utils.unwrapObservable(currentContact.EmailAddress);

        self.ValidationErrors.removeAll(); // Clear out any previous errors

        if (!currentFirstName)
            self.ValidationErrors.push("First name is required.");

        if (!currentLastName)
            self.ValidationErrors.push("Last name is required.");

        if (!currentEmailAddress)
            self.ValidationErrors.push("Email address is required.");

        return self.ValidationErrors().length <= 0;
    };

    function getAllContacts() {
        ajaxHelper(contactsUri, 'GET').done(function (data) {
            self.Contacts(data);
        });
    }

    self.OriginalContactInstance = ko.observable();

    getAllContacts();
    self.AddNewContact = function () {
        self.ContactBeingEdited(new Contact());
        self.OriginalContactInstance(undefined);
    };
    self.ValidationErrors = ko.observableArray([]);

    self.ContactBeingEdited = ko.observable();
    self.EditContact = function (contact) {
        self.OriginalContactInstance(contact);
        self.ContactBeingEdited(new Contact(contact.Id, contact.FirstName, contact.LastName, contact.EmailId, contact.EmailAddress, contact.EmailAddressType));
    };

    self.SaveContact = function() {
        const updatedContact = ko.utils.unwrapObservable(self.ContactBeingEdited);
        if (!self.ValidateContact(updatedContact)) {
            return false;
        } else {
            const id = ko.utils.unwrapObservable(updatedContact.Id);
            const firstName = ko.utils.unwrapObservable(updatedContact.FirstName);
            const lastName = ko.utils.unwrapObservable(updatedContact.LastName);
            const emailId = ko.utils.unwrapObservable(updatedContact.EmailId);
            const emailAddress = ko.utils.unwrapObservable(updatedContact.EmailAddress);
            const emailAddressType = ko.utils.unwrapObservable(updatedContact.EmailAddressType);

            const newContact = new Contact(id, firstName, lastName, emailId, emailAddress, emailAddressType);

            if (self.OriginalContactInstance() === undefined) {

                ajaxHelper('/api/Contact/' + '/PostContact/', 'POST', ko.toJSON(newContact)).done(function(item) { 
                        if (!item.success) {
                            self.ValidationErrors.push(item.errors);
                        } else {
                            self.Contacts.push(newContact);
                            $('.close').click(); 
                        }
                });
            } else {
                ajaxHelper('/api/Contact/' + 'PutContact/', 'PUT', ko.toJSON(newContact)).done(function (item) {
                    if (!item.success) {
                        self.ValidationErrors.push(item.errors);
                    } else {
                        getAllContacts();
                        $('.close').click(); 
                    } 
                });
            } 
          
        }
        self.ContactBeingEdited(undefined);
        self.OriginalContactInstance(undefined);
    };
};

ko.applyBindings(ViewModel());

ko.bindingHandlers['modal'] = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        "use strict";
        var allBindings = allBindingsAccessor();
        const $element = $(element);
        $element.addClass('hide modal');
        if (allBindings.modalOptions && allBindings.modalOptions.beforeClose) {
            $element.on('hide', function () {
                const value = ko.utils.unwrapObservable(valueAccessor());
                return allBindings.modalOptions.beforeClose(value);
            });
        }
    },
    update: function (element, valueAccessor) {
        const value = ko.utils.unwrapObservable(valueAccessor());
        if (value) {
            $(element).removeClass('hide').modal('show');
        } else {
            $(element).modal('hide');
        }
    }
};

$('#FileUpload').click(function () {
    var  fileToRead = document.getElementById('ContactsFile');
    if (fileToRead.files.length > 0) {
        var reader = new FileReader(); 
        reader.onload = Load_CSVData;
        reader.readAsText(fileToRead.files.item(0));
    }
}); 

function ajaxHelper(uri, method, data) { 
    return $.ajax({
        type: method,
        url: uri,
        dataType: 'json',
        contentType: 'application/json',
        data: data ? data : null
    }).fail(function (jqXHR, textStatus, errorThrown) {
        error(errorThrown);
    });
}

function Load_CSVData(e) {
    window.CSVLines  = e.target.result.split(/\r\n|\n/);
    $.each(window.CSVLines, function (i, item) {
       
        const element = item.split(",");
        const firstName = (element[0] == undefined) ? "" : element[0].trim();
        const lastName = (element[1] == undefined) ? "" : element[1].trim();
        const emailAddress = (element[2] == undefined) ? "" : element[2].trim();
        const emailAddressType = (element[3] == undefined) ? "" : element[3].trim();

        if (firstName === "" || lastName === "" || emailAddress === "" || emailAddressType === "")  //Ignore bad data, for now....
            return; 

        self.CSVContacts.push(
            { 
                Id : 0,
                FirstName: firstName,
                LastName: lastName,
                EmailId: 0,
                EmailAddress: emailAddress, 
                EmailAddressType: emailAddressType
            }  
        );
    });

    ajaxHelper('/api/Contact/' + '/PostContacts/', 'POST', ko.toJSON(self.CSVContacts)).done(function (item) {
        if (!item.success) {
            error.push(item.errors);
        } else {
            location.reload(); 
        } 
    });
}

