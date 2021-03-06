require 'rspec'
require_relative 'Head/sdk_requirements'

include MyNamespace

describe 'Head' do
  before(:all) do
    @base_url = ENV['StubServerURI']

    dummyToken = 'dummy12321343423'
    dummySubscription = '1-1-1-1'
    @credentials = MsRestAzure::TokenCloudCredentials.new(dummySubscription, dummyToken)

    @client = AutoRestHeadTestService.new(@credentials, @base_url)
  end

  it 'send head 204' do
    result = @client.http_success.head204().value!
    expect(result.body).to be(true)
  end

  it 'send head 404' do
    result = @client.http_success.head404().value!
    expect(result.body).to be(false)
  end
end