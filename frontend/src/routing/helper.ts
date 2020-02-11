
import { RouteConfig } from 'aurelia-router';

export type RouteConfigWithName = RouteConfig & { name: string }

export function route(routeConfig: RouteConfig): RouteConfigWithName {
  return routeConfig as RouteConfigWithName;
}

export function mapRouteDictionary(routeDictionary: { [name: string]: RouteConfig }, routeRoot: string, filter: (route: RouteConfig) => boolean = _route => true): RouteConfig[] {
  let routeNames = Object.keys(routeDictionary);
  let routes = routeNames.map<RouteConfigWithName>(name => mapRoute(routeDictionary, name, routeRoot));
  routes = filterRouteTree(routes, filter);
  return routes;
}

function mapRoute(routeDictionary: { [name: string]: RouteConfig }, name: string, routeRoot: string) {
  const routeConfig = routeDictionary[name];
  routeConfig.name = name;

  if (typeof routeConfig.route === 'string') {
    routeConfig.route = routeRoot + routeConfig.route;
  } else if (Array.isArray(routeConfig.route)) {
    routeConfig.route = routeConfig.route.map((route: string) => routeRoot + route);
  }

  if (typeof routeConfig.redirect == 'string') {
    routeConfig.redirect = routeRoot + routeConfig.redirect;
  }

  return routeConfig as RouteConfigWithName;
}

function filterRouteTree(routes: RouteConfigWithName[], filter: (route: RouteConfig) => boolean) {
  return routes.filter(route => {
    if (!filter(route)) {
      return false;
    }

    let submenuItems = setting(route, 'submenuItems');
    if (submenuItems) {
      submenuItems = filterRouteTree(submenuItems, filter);
      setting(route, 'submenuItems', submenuItems);

      if (setting(route, 'firstChildAsRoute') && submenuItems && submenuItems.length > 0) {
        route.route = submenuItems[0].route;
        route.moduleId = submenuItems[0].moduleId;
      }
    }

    if (setting(route, 'hideIfNoSubmenuItems') && (!submenuItems || submenuItems.length === 0)) {
      return false;
    }

    return true;
  });
}

export function topRoutes(...routes: RouteConfig[]) {
  for (let route of routes) {
    route.nav = true;
  }
}

export function submenuItems(parentRoute: RouteConfig, subroutes?: RouteConfig[]) {
  if (subroutes == null) {
    return parentRoute && parentRoute.settings && parentRoute.settings.submenuItems || [];
  } else {
    setting(parentRoute, 'submenuItems', subroutes);
    parent(parentRoute, subroutes);
  }
}

export function parentRoute(parentRoute: RouteConfig, subroutes: RouteConfig[]) {
  parent(parentRoute, subroutes);
}

function parent(parentRoute: RouteConfig, subroutes: RouteConfig[]) {
  for (let subroute of subroutes) {
    setting(subroute, 'parentRoute', parentRoute);
  }
}

function setting(route: RouteConfig, property: string, value?: any) {
  if (typeof value === 'undefined') {
    return route.settings && route.settings[property];
  }
  if (!route.settings) {
    route.settings = {};
  }
  route.settings[property] = value;
}
