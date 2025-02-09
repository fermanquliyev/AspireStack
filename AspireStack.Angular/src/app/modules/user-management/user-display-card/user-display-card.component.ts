import { Component, Input, OnInit } from '@angular/core';
import { CardComponent, CardFooterComponent, CardHeaderComponent, ListGroupDirective, ListGroupItemDirective } from '@coreui/angular';
import { UserDto } from 'src/app/services/api-services/api-service-proxies';

@Component({
  selector: 'app-user-display-card',
  templateUrl: './user-display-card.component.html',
  styleUrls: ['./user-display-card.component.css'],
  imports: [
    CardHeaderComponent,
    CardFooterComponent,
    CardComponent,
    ListGroupDirective,
    ListGroupItemDirective
  ]
})
export class UserDisplayCardComponent implements OnInit {

  @Input() header: string = '';
  @Input() footer: string = '';
  @Input() user!: UserDto;
  constructor() { }

  ngOnInit() {
  }

}
