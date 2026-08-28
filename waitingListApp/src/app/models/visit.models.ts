export interface Centre {
  centreId: number;
  externalCentreId: string;
  centreName: string;
  address: string;
  latitude: number;
  longitude: number;
  allowedRadiusMetres: number;
}

export interface CreateVisitRequest {
  accNumber: string;
  fullName: string;
  cellNumber: string;
  centreId: number;
  visitDate: string;
}

export interface Visit {
  visitId: string;
  accNumber: string;
  fullName: string;
  cellNumber: string;
  centreId: number;
  centreName: string;
  visitDate: string;
  status: string;
  createdAt: string;
  qrToken: string;
  qrGeneratedAt: string;
}
