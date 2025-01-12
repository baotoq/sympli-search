"use client";

import {
  DeleteButton,
  EditButton,
  ImageField,
  List,
  ShowButton,
  useTable,
} from "@refinedev/antd";
import { useInvalidate, type BaseRecord, type useCustom } from "@refinedev/core";

import { Space, Table, Form, Input, Button, Select } from "antd";
import { useState } from "react";

export default function SearchList() {
  const invalidate = useInvalidate();

  const { tableProps } = useTable({
    syncWithLocation: true,
  });

  const [url, setUrl] = useState("www.sympli.com.au");
  const [keyword, setKeyword] = useState("e-settlements");
  const [searchEngine, setSearchEngine] = useState(0);

  const handleKeywordChange = (event: any) => {
    setKeyword(event.target.value);
  };

  const handleUrlChange = (event: any) => {
    setUrl(event.target.value);
  };

  const handleSearchEngineChange = (value: any) => {
    setSearchEngine(value);
  };

  async function handleOnClick(event: any): Promise<void> {
    try {
      event.preventDefault();
      const auth = localStorage.getItem("token");
      const response = await fetch(
        `http://localhost:5413/api/seo?keyword=${encodeURIComponent(
          keyword
        )}&url=${encodeURIComponent(url)}&searchEngineType=${encodeURIComponent(searchEngine)}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: "Bearer " + auth,
          },
        }
      );
      const data = await response.json();

      console.log(data);

      invalidate({
        resource: "search-histories",
        invalidates: ["list"],
      });
      
    } catch (error) {
      console.error("Error fetching data:", error);
    }
  }

  return (
    <>
      <Form layout="vertical">
        <Form.Item label="Keyword" name="keyword">
          <Input onChange={handleKeywordChange} />
        </Form.Item>
        <Form.Item label="Url" name="url">
          <Input onChange={handleUrlChange} />
        </Form.Item>
        <Form.Item label="Search Engine" name="searchEngine">
          <Select onChange={handleSearchEngineChange}
            options={[
              { value: 0, label: <span>Google</span> },
              { value: 1, label: <span>Bing</span> },
            ]}
          />
        </Form.Item>
        <Button onClick={handleOnClick}>Search</Button>
      </Form>
      <div className="mt-5"></div>
      <List>
        <Table {...tableProps} rowKey="id">
          <Table.Column dataIndex="keyword" title={"Keyword"} />
          <Table.Column dataIndex="url" title={"Url"} />
          <Table.Column dataIndex="positions" title={"Positions"} />
          <Table.Column
            title={"Actions"}
            dataIndex="actions"
            render={(_, record: BaseRecord) => (
              <Space>
                <EditButton hideText size="small" recordItemId={record.id} />
                <ShowButton hideText size="small" recordItemId={record.id} />
                <DeleteButton hideText size="small" recordItemId={record.id} />
              </Space>
            )}
          />
        </Table>
      </List>
    </>
  );
}
