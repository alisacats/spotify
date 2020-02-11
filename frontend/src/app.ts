import { autoinject } from 'aurelia-framework';
import { RouterConfiguration, Router, NavigationInstruction, Next } from 'aurelia-router';
import { configureRoutes } from 'routing/routes-cfg';

@autoinject
export class App {
  async configureRouter(config: RouterConfiguration, _router: Router) {
    config.title = 'Spotify Bot';
    configureRoutes(config, '/');

    // scroll to top after navigating, https://github.com/aurelia/router/issues/170#issuecomment-359535312
    config.addPostRenderStep({
      run(navigationInstruction: NavigationInstruction, next: Next) {
        if (navigationInstruction.router.isNavigatingNew) {
          window.scroll(0, 0);
        }
        return next();
      }
    });
  }
}
