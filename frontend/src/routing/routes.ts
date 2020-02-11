import { route, topRoutes, parentRoute } from './helper';
import { PLATFORM } from 'aurelia-pal';

const layout = PLATFORM.moduleName('views/layout');

export const Routes = {
  home: route({
    title: 'Home',
    route: '',
    moduleId: PLATFORM.moduleName('views/account-list'),
    layoutViewModel: layout
  }),

  account: route({
    title: 'Account Management',
    route: 'account/:accountId',
    moduleId: PLATFORM.moduleName('views/account'),
    layoutViewModel: layout
  }),
}

topRoutes(
  Routes.home,
);
parentRoute(Routes.home, [ Routes.account ]);
