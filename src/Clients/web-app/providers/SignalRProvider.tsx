"use client";

import { User } from "next-auth";
import { useParams } from "next/navigation";
import toast from "react-hot-toast";
import { ReactNode, useRef, useEffect, useCallback } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

import { useAuctionStore, useBidStore } from "@/hooks";
import { Auction, AuctionFinished, Bid } from "@/types";

import { AuctionCreatedToast, AuctionFinishedToast } from "@/components";
import { getAuctionById } from "@/services";

type Props = {
  children: ReactNode;
  user: User | null;
};

export const SignalRProvider = ({ children, user }: Props) => {
  const connection = useRef<HubConnection | null>(null);
  const { setCurrentPrice } = useAuctionStore();
  const { addBid } = useBidStore();
  const params = useParams<{ id: string }>();

  const handleAuctionFinished = useCallback(
    (finishedAuction: AuctionFinished) => {
      const auction = getAuctionById(finishedAuction.auctionId);
      return toast.promise(
        auction,
        {
          loading: "Loading",
          success: (auction) => (
            <AuctionFinishedToast
              finishedAuction={finishedAuction}
              auction={auction}
            />
          ),
          error: () => "Auction finished",
        },
        { success: { duration: 10000, icon: null } }
      );
    },
    []
  );

  const handleAuctionCreated = useCallback(
    (auction: Auction) => {
      if (user?.username !== auction.seller) {
        return toast(<AuctionCreatedToast auction={auction} />, {
          duration: 10000,
        });
      }
    },
    [user]
  );

  const handleBidPlaced = useCallback(
    (bid: Bid) => {
      if (bid.bidStatus.includes("Accepted")) {
        setCurrentPrice(bid.auctionId, bid.amount);
      }

      if (params.id === bid.auctionId) {
        addBid(bid);
      }
    },
    [setCurrentPrice, addBid, params.id]
  );

  useEffect(() => {
    if (!connection.current) {
      connection.current = new HubConnectionBuilder()
        .withUrl("http://localhost:6001/notifications")
        .withAutomaticReconnect()
        .build();

      connection.current
        .start()
        .then(() => console.log("Connected to notification hub"))
        .catch((err) => console.error("Error connecting to SignalR hub:", err));
    }

    connection.current.on("BidPlaced", handleBidPlaced);
    connection.current.on("AuctionCreated", handleAuctionCreated);
    connection.current.on("AuctionFinished", handleAuctionFinished);

    return () => {
      connection.current?.off("BidPlaced", handleBidPlaced);
      connection.current?.off("AuctionCreated", handleAuctionCreated);
      connection.current?.off("AuctionFinished", handleAuctionFinished);
    };
  }, [handleBidPlaced, handleAuctionCreated, handleAuctionFinished]);

  return <>{children}</>;
};
