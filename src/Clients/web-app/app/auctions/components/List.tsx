"use client";

import { useEffect, useState } from "react";

import qs from "query-string";

import { getAuctionsList } from "@/services";
import { DefaultsDisplay, AppPagination } from "@/components";
import { useParamsStore } from "@/hooks";
import { Auction, PagedResult } from "@/types";

import { AuctionCard, Filters } from ".";

export const AuctionsList = () => {
  const [data, setData] = useState<PagedResult<Auction>>();
  const {
    pageNumber,
    pageSize,
    searchTerm,
    orderBy,
    filterBy,
    seller,
    winner,
    setParams,
  } = useParamsStore();

  const url = qs.stringifyUrl({
    url: "",
    query: {
      pageNumber,
      pageSize,
      searchTerm,
      orderBy,
      filterBy,
      seller,
      winner,
    },
  });

  function setPageNumber(pageNumber: number) {
    setParams({ pageNumber });
  }

  useEffect(() => {
    getAuctionsList(url).then((data) => {
      setData(data);
    });
  }, [url]);

  if (!data) return <h3>Loading...</h3>;

  return (
    <>
      <Filters />
      {data.totalCount === 0 ? (
        <DefaultsDisplay showReset />
      ) : (
        <>
          <div className="grid grid-cols-4 gap-6">
            {data.results.map((auction) => (
              <AuctionCard auction={auction} key={auction.id} />
            ))}
          </div>
          <div className="flex justify-center mt-4">
            <AppPagination
              pageChanged={setPageNumber}
              currentPage={pageNumber}
              pageCount={data.pageCount}
            />
          </div>
        </>
      )}
    </>
  );
};
