import { autoinject, computedFrom } from 'aurelia-framework';
import { Router } from 'aurelia-router';

@autoinject
export class Layout {
  constructor(
    private readonly router: Router
  ) { }

  @computedFrom('router.currentInstruction')
  get title() {
    const instruction = this.router.currentInstruction;
    return instruction && instruction.config.settings && instruction.config.settings.header1 || instruction.config.title;
  }
}
