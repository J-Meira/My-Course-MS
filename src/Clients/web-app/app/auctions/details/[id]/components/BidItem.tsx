import { format } from "date-fns";

import { numberWithCommas } from "@/libs";
import { Bid } from "@/types";

type Props = {
  bid: Bid;
};

export const BidItem = ({ bid }: Props) => {
  const getBidInfo = () => {
    switch (bid.bidStatus) {
      case "Accepted":
        return {
          bgColor: "bg-green-200",
          text: "Bid accepted",
        };

      case "AcceptedBelowReserve":
        return {
          bgColor: "bg-amber-200",
          text: "Reserve not met",
        };

      case "TooLow":
        return {
          bgColor: "bg-red-200",
          text: "Bid was too low",
        };

      default:
        return {
          bgColor: "bg-red-200",
          text: "Bid placed after auction finished",
        };
    }
  };

  return (
    <div
      className={`w-full border-gray-300 border-2 px-3 py-2 rounded-lg flex justify-between items-center mb-2 ${
        getBidInfo().bgColor
      }`}
    >
      <div className="flex flex-col">
        <span>Bidder: {bid.bidder}</span>
        <span className="text-gray-700 text-sm">
          Time: {format(bid.bidTime, "dd MMM yyyy h:mm a")}
        </span>
      </div>
      <div className="flex flex-col text-right">
        <div className="text-xl font-semibold">
          ${numberWithCommas(bid.amount)}
        </div>
        <div className="flex flex-row items-center">
          <span>{getBidInfo().text}</span>
        </div>
      </div>
    </div>
  );
};
