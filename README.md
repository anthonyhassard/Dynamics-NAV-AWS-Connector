Dynamics-NAV-AWS-Connector
==========================

A Dynamics-NAV friendly .NET library providing simplified connectors to various Amazon Web Services. The intent is to commit often, warts and all.  I will put really rough stuff in a branch and keep the trunk relatively clean, though overall this project is really more of an intellectual exercise than anything intended for production use.

Eventually I'd like to use the AWS libraries in at least 2 ways: first, I'd like to use the services (especially the Queue and load balancing solutions) to buffer data such as customers, items, etc. This buffer will provide easier access to NAV master data for external systems without having to be concerned about specific NAV implementation details -- essentially, a "normalization" of NAV data to make it more readily available for POS, CRM, web store, and many other types of consumers.

Second, I would like to build a distributed source control system to handle NAV objects as cleanly as possible.  Again, the AWS queue can potentially serve as a good buffer between the development systems and the source control system, so that neither one becomes dependent on the other.  It might also be nice to use the AWS notification services to alert other developers of changes.

Again, this project is an intellectual exercise and hopefully might give other developers a sense of direction -- an example of one way (certainly not close to the best way) to distribute NAV data in a way that reduces NAV-specific dependencies for external interfaces.