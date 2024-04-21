import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {MatTabsModule} from '@angular/material/tabs';
import { DeviceService } from '../../../core/services/http/device.service';
import { AuthService } from '../../../core/services/http/auth.service';

@Component({
  selector: 'app-map-filter',
  standalone: true,
  imports: [CommonModule, FormsModule, MatTabsModule],
  templateUrl: './map-filter.component.html',
  styleUrl: './map-filter.component.scss'
})
export class MapFilterComponent {
  searchQuery: string = '';
  allDevices: any[] = [];
  searchedDevices: any[] = [];

  @Input() filteredDevices: any[] = [];
  @Output() closedFilter = new EventEmitter<void>();
  companyId : number = 0;
  @Output() zoomEvent = new EventEmitter<number>();

  activeDeviceId: number | null = null; 
  filtered: any[] | undefined;
  searchDevices(): void {
    
    this.filtered = this.filteredDevices;
    this.searchedDevices = this.filtered.filter(device => 
      device.deviceName.toLowerCase().includes(this.searchQuery.toLowerCase())
    );
  }
  zoomToSpecificPoint(deviceID: number) {
    this.zoomEvent.emit(deviceID);
  }
  onMarkerClicked(deviceID: number) {
    
    console.log('Marker clicked:', deviceID);
    
  }
  toggleActiveDevice(deviceId: number) {
    if (this.activeDeviceId === deviceId) {
      
      this.activeDeviceId = null;
    } else {
      
      this.activeDeviceId = deviceId;
    }
  }

 
  isDeviceActive(deviceId: number) {
    return this.activeDeviceId === deviceId;
  }

  constructor(private deviceService: DeviceService, private authService: AuthService){
    this.authService.getCurrentUser().subscribe((res : any) => {
      this.companyId = res.companyID;
      this.getAll(res.companyID);
      
    })
    
  }
  
  selectedView: string = 'view1';

  switchView() {}

  closeFilterComponent(): void {
    this.closedFilter.emit();
  }

  getAll(companyId : number): void {
    this.deviceService.getCompanyDevices(companyId).subscribe(devices => {
      this.filteredDevices = devices;
      
    });
  }

  
}
