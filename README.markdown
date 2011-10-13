# CLOUD STorage ABstraction

The goal behind Cloudstab is to provide a set of interfaces that will allow access to and usage of basic cloud storage facilities.  The initial motivation behind this project is to allow applications built against Amazon's S3 to use local storage during development.  Because Azure already has facilities for this, I decided to make the interface generic enough to work against both S3 and Azure Blob storage which means that I will be able to leverage Azure's development infrastructure.

Creating an abstraction for multiple cloud providers' storage implementations means that I will only be able to support the lowest common denominator.  In this case, I think this will be enough as both Amazon and Microsoft have a concept of a container and an object.
