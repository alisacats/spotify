import { RouterConfiguration } from 'aurelia-router';
import { Routes } from './routes';
import { mapRouteDictionary } from './helper';

export function configureRoutes(
  config: RouterConfiguration,
  routeRoot: string
) {
  config.options.pushState = true;
  config.map(mapRouteDictionary(Routes, routeRoot));
}
