import { Component, EventEmitter, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AddNewDeviceComponent } from './add-new-device/add-new-device.component';
import { DeviceService } from '../../core/services/http/device.service';
import { DeviceRequest } from '../../core/models/device-request';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-devices',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './devices.component.html',
  styleUrl: './devices.component.scss'
})
export class DevicesComponent {
  @Output() deviceDeleted: EventEmitter<any> = new EventEmitter<any>();
  
  modalVisible: boolean = false;
  devices: any[] = [];
  deviceRequest: DeviceRequest = {
    userID: 0
  };

  constructor(public dialog: MatDialog,
    private deviceService: DeviceService) { }

  ngOnInit(): void {
    this.getAll();
  }
  openDialog(): void {
    const dialogRef = this.dialog.open(AddNewDeviceComponent, {
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
    dialogRef.componentInstance.deviceAdded.subscribe(() => {
      this.getAll(); // Refresh table after user is added
    });
  }
  edit(device: any): void {
    const deviceId=device.id;
    this.deviceService.updateDevice(this.deviceRequest,deviceId).subscribe(() => {
      console.log('Device updated successfully');
      this.getAll();
    });
  }

  delete(device:any, event: Event): void {
    console.log(device);
    const deviceId=device.deviceID;
    event.preventDefault();
    this.deviceService.deleteDevice(deviceId).subscribe(() => {
      this.deviceDeleted.emit();
      console.log('Device deleted successfully');
      this.getAll();
    });

  }

  getAll(): void {
    this.deviceService.getCompanyDevices().subscribe(devices => {
      this.devices = devices;
    });
  }
}
