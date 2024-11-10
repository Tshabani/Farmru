import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AppComponent } from './app.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    {
                        path: 'home',
                        loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
                        data: { permission: 'Pages.Home' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'about',
                        loadChildren: () => import('./about/about.module').then((m) => m.AboutModule),
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'users',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'roles',
                        loadChildren: () => import('./roles/roles.module').then((m) => m.RolesModule),
                        data: { permission: 'Pages.Roles' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'tenants',
                        loadChildren: () => import('./tenants/tenants.module').then((m) => m.TenantsModule),
                        data: { permission: 'Pages.Tenants' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'update-password',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'node',
                        loadChildren: () => import('./node/node.module').then((m) => m.NodeModule),
                        data: { permission: 'Pages.Nodes' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'person',
                        loadChildren: () => import('./person/person.module').then((m) => m.PersonModule),
                        data: { permission: 'Pages.People' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'facility',
                        loadChildren: () => import('./facility/facility.module').then((m) => m.FacilityModule),
                        data: { permission: 'Pages.Facilities' },
                        canActivate: [AppRouteGuard]
                    },
                    {
                        path: 'facility-appointment',
                        loadChildren: () => import('./facility-appointment/facility-appointment.module').then((m) => m.FacilityAppointmentModule),
                        data: { permission: 'Pages.Facility.Appointments' },
                        canActivate: [AppRouteGuard]
                    },
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
