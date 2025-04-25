EmailCore (Class Library)

Contains the core email sending logic.

Handles sending emails asynchronously.

Reads SMTP settings from a configuration.

EmailSenderConsoleApp (Console Application)

A simple console application to demonstrate sending an email using EmailCore.

EmailSenderApi (ASP.NET Core Web API)

Provides an API endpoint to trigger email sending using EmailCore.

The API will be set as the startup project.

EmailServiceWebApp (ASP.NET Core MVC Web Application)

A web application that calls the EmailSenderApi to send emails.
