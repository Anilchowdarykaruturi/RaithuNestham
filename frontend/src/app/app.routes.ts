import { Routes } from '@angular/router';

import { authGuard } from './guards/auth-guard';

export const routes: Routes = [

{
path: '',
redirectTo: 'login',
pathMatch: 'full'
},

{
path: 'login',
loadComponent: () =>
import('./pages/login/login')
.then(m => m.Login)
},

{
path: 'register',
loadComponent: () =>
import('./pages/register/register')
.then(m => m.Register)
},

{
path: 'dashboard',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/dashboard/dashboard')
.then(m => m.Dashboard)
},

{
path: 'weather',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/weather/weather')
.then(m => m.Weather)
},

{
path: 'crops',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/crops/crops')
.then(m => m.Crops)
},

{
path: 'fertilizers',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/fertilizers/fertilizers')
.then(m => m.Fertilizers)
},

{
path: 'pesticides',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/pesticides/pesticides')
.then(m => m.Pesticides)
},

{
path: 'government-schemes',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/government-schemes/government-schemes')
.then(m => m.GovernmentSchemes)
},

{
path: 'equipment-subsidies',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/equipment-subsidies/equipment-subsidies')
.then(m => m.EquipmentSubsidiesComponent)
},

{
path: 'ai-chat',
canActivate: [authGuard],
loadComponent: () =>
import('./pages/ai-chat/ai-chat')
.then(m => m.AiChat)
},

{
path: '**',
redirectTo: 'login'
}

];
