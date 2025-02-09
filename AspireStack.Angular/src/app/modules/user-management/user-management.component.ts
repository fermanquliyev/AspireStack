import { Component, OnInit, signal } from '@angular/core';
import * as coreui from '@coreui/angular';
import { ApiService, GetUsersInput, UserDto, UserDtoPagedResult } from '../../services/api-services/api-service-proxies';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgFor } from '@angular/common';
import "@angular/localize/init";
import { CreateEditUserModalComponent } from './create-edit-user-modal/create-edit-user-modal.component';


@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
  imports: [coreui.RowComponent, 
    coreui.ColComponent, 
    coreui.TextColorDirective, 
    coreui.CardComponent, 
    coreui.CardHeaderComponent, 
    coreui.CardBodyComponent, 
    coreui.TableDirective,
    coreui.ButtonDirective,
    coreui.PaginationComponent,
    CreateEditUserModalComponent,
    DatePipe,
    FormsModule,
    NgFor
  ]
})
export class UserManagementComponent implements OnInit {


  public page = 1;
  public pageSize = 10;
  public totalPages = 0;
  public filter = signal('');
  users: UserDtoPagedResult = new UserDtoPagedResult();
  constructor(
    private client: ApiService
  ) { }

  ngOnInit() {
  this.getUsers();
  }

  getUsers(){
    this.client.getUsers(
      new GetUsersInput({
        filter: this.filter(),
        page: this.page,
        pageSize: this.pageSize
      })
    ).subscribe(pagedResult => {
      this.users = pagedResult;
      this.totalPages = Math.ceil((pagedResult.totalCount ?? 0) / this.pageSize);
    })
  }

  trackByUserId(index: number, user: UserDto) {
    return user.id;
  }

  onPageChange(page: number) {
    this.page = page;
    this.getUsers();
  }

  onPageSizeChange(pageSize: number | Event) {
    if (pageSize instanceof Event) {
      pageSize = parseInt((pageSize.target as HTMLSelectElement).value);
    }
    this.pageSize = pageSize;
    this.getUsers();
  }

  public L(text: string) {
    return $localize`${text}`;
  }
}
