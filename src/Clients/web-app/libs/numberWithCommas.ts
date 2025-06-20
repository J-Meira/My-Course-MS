export const numberWithCommas = (amount: number) =>
  amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
