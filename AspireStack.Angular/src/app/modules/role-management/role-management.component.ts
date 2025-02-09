import { Component, OnInit, signal } from '@angular/core';
import * as coreui from '@coreui/angular';
import { ApiService, GetUsersInput, RoleDtoPagedResult, UserDto, UserDtoPagedResult, PagedResultRequest, RoleDto } from '../../services/api-services/api-service-proxies';
import { FormsModule } from '@angular/forms';
import { DatePipe, NgFor } from '@angular/common';
import "@angular/localize/init";
import { CreateEditRoleModalComponent } from './create-edit-role-modal/create-edit-role-modal.component';


@Component({
  selector: 'app-role-management',
  templateUrl: './role-management.component.html',
  styleUrls: ['./role-management.component.css'],
  imports: [coreui.RowComponent, 
    coreui.ColComponent, 
    coreui.TextColorDirective, 
    coreui.CardComponent, 
    coreui.CardHeaderComponent, 
    coreui.CardBodyComponent, 
    coreui.TableDirective,
    coreui.ButtonDirective,
    coreui.PaginationComponent,
    CreateEditRoleModalComponent,
    DatePipe,
    FormsModule,
    NgFor
  ]
})
export class RoleManagementComponent implements OnInit {


  public page = 1;
  public pageSize = 10;
  public totalPages = 0;
  roles: RoleDtoPagedResult = new RoleDtoPagedResult();
  constructor(
    private client: ApiService
  ) { }

  ngOnInit() {
  this.getRoles();
  }

  getRoles(){
    this.client.getAllRolesPaged(new PagedResultRequest({
        page: this.page,
        pageSize: this.pageSize
      })
    ).subscribe(pagedResult => {
      this.roles = pagedResult;
      this.totalPages = Math.ceil((pagedResult.totalCount ?? 0) / this.pageSize);
    })
  }

  trackByRoleId(index: number, role: RoleDto) {
    return role.id;
  }

  onPageChange(page: number) {
    this.page = page;
    this.getRoles();
  }

  onPageSizeChange(pageSize: number | Event) {
    if (pageSize instanceof Event) {
      pageSize = parseInt((pageSize.target as HTMLSelectElement).value);
    }
    this.pageSize = pageSize;
    this.getRoles();
  }

  public L(text: string) {
    return $localize`${text}`;
  }
}
