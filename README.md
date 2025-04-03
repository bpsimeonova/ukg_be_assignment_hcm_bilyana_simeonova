# HCM
There are 4 roles - admin, hr, manager, employee. Depending on the role users have different rights and access.
Two initial users are added - admin and hr. 
For login users need username and password.
When a new user is created, the default username is formed as follows firstName.lastName@hcm.com, the default password is "User123*" (user can change it later from "Change password".)
If users have no manager or they have subordinates, then they are also in role "manager", e.g. the default created admin is in roles "admin" and "manager".
Users in role "admin" can add hr-s. They can see data about all employees, but cannot edit or delete any. They can see their own leaves.
Users in role "hr" can add and edit job titles and employees. They can view all leaves, but can edit only their own.
Users in role "manager" can approve or reject leave requests.
All users can see the company organizational chart, their line manager and who is off today. They can also edit their public profile and change the password.
Job titles, employees can be deleted only if they are not associated with other employees or leaves.
