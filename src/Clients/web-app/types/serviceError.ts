type TError = {
  status: number;
  message: string;
};

export type ServiceError = {
  error: TError;
};
