export interface CheckInRequest {
  qrToken: string;
  latitude: number;
  longitude: number;
}

export interface CheckInResult {
  scanSuccessful: boolean;
  message: string;
  centreName: string;
  visitDate: string;
  appointmentStatus: string;
  queueNumber: string | null;
  queueStatus: string | null;
  peopleAhead: number | null;
  joinedAt: string | null;
}
