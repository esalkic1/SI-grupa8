import { Component, EventEmitter, Output } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { DeviceRequest } from '../../../core/models/device-request';
import { DeviceService } from '../../../core/services/http/device.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { CodeInputModule } from 'angular-code-input';
import { UserRequest } from '../../../core/models/user-request';
import { CompanyService } from '../../../core/services/http/company.service';
import { UserService } from '../../../core/services/http/user.service';
import { DeviceType } from '../../../core/models/device-type';
import { AuthService } from '../../../core/services/http/auth.service';

@Component({
  selector: 'app-add-new-device',
  standalone: true,
  imports: [FormsModule,NgIf, ReactiveFormsModule, CodeInputModule, CommonModule],
  templateUrl: './add-new-device.component.html',
  styleUrl: './add-new-device.component.scss'
})
export class AddNewDeviceComponent {
  @Output() deviceAdded: EventEmitter<any> = new EventEmitter<any>();

  addDeviceForm: FormGroup;
  deviceRequest: DeviceRequest = {
  };

  users : UserRequest[] = []
  deviceTypes : DeviceType[] = []

  constructor(public f: FormBuilder,public dialogRef: MatDialogRef<AddNewDeviceComponent>, private deviceService: DeviceService, private userService : UserService,
    private authService : AuthService
  ) {
    this.addDeviceForm = this.f.group({
      deviceName: [''],
      user: [''],
      ref: [''],
      xcoord: [''], 
      ycoord: [''],
      userId: [''],
      deviceTypeId: ['']
    });
    this.userService.getUser().subscribe((res: any) => {
      this.getAllUsers(res.companyID);
    })
    this.getAllDeviceTypes();
  }



  closeDialog(): void {
    this.dialogRef.close();
  }
  add(event: Event){
    this.deviceRequest.deviceName = this.addDeviceForm.get('deviceName')?.value;
      this.deviceRequest.reference = this.addDeviceForm.get('ref')?.value;
      //hardkodiran userID
      //this.deviceRequest.userID=1;
      this.deviceRequest.xCoordinate = this.addDeviceForm.get('xcoord')?.value;
      this.deviceRequest.yCoordinate = this.addDeviceForm.get('ycoord')?.value;
      this.deviceRequest.userID = this.addDeviceForm.get('userId')?.value;
      this.deviceRequest.deviceTypeID = this.addDeviceForm.get('deviceTypeId')?.value;
      event.preventDefault();
    this.deviceService.createDevice(this.deviceRequest).subscribe(()=>{
      this.deviceAdded.emit();
      console.log('Device added successfully');
      this.closeDialog();
    });
  }

  getAllUsers(companyID : number){
    this.userService.getDispatchersForNewDevice(companyID).subscribe(x => {
      this.users = x;
    })
  }

  getAllDeviceTypes(){
    this.deviceService.getDeviceTypes().subscribe(x => {
      this.deviceTypes = x;
    })
  }

}