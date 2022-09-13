using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRCToolBox.Common
{
    // reference : https://takap-tech.com/entry/2018/01/02/044400
    internal class RingBuffer<T> : IEnumerable<T>
    {
        private readonly Queue<T> _queue;
        /// <summary>
        /// バッファーに格納されている要素の数を取得します。
        /// </summary>
        public int Count => this._queue.Count;

        /// <summary>
        /// このバッファーの最大容量を取得します。
        /// </summary>
        public int MaxCapacity { get; private set; }

        /// <summary>
        /// 指定した位置の要素を参照します。
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index > this.Count)
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)}={index}");
                return _queue.ElementAt(index);
            }
        }
        /// <summary>
        /// リングバッファーの最大要素数をしていしてオブジェクトを初期化します。
        /// </summary>
        public RingBuffer(int maxCapacity)
        {
            this.MaxCapacity = maxCapacity;
            _queue = new Queue<T>(maxCapacity);
        }

        // -x-x- Public Methods -x-x-

        /// <summary>
        /// 指定した要素をリングバッファーに追加します。
        /// </summary>
        public void Add(T item)
        {
            _queue.Enqueue(item);
            if (_queue.Count > this.MaxCapacity)
            {
                T removed = this.Pop();

                // デバッグ用の出力:
                // Console.WriteLine($"キャパシティを超えているためバッファの先頭データ破棄しました。{removed}");
            }
        }

        /// <summary>
        /// 末尾の要素を取得しバッファーからデータを削除します。
        /// </summary>
        public T Pop() => _queue.Dequeue();

        /// <summary>
        /// バッファーの先頭の要素を取得します。データは削除されません。
        /// </summary>
        public T First() => _queue.Peek();

        /// <summary>
        /// 指定した要素が存在するかどうかを確認します。
        /// </summary>
        public bool Contains(T item) => _queue.Contains(item);

        /// <summary>
        /// このオブジェクトが管理中の全データを配列として取得します。
        /// </summary>
        public T[] ToArray() => _queue.ToArray();

        /// <summary>
        /// 現在のリングバッファー内の要素を列挙します。
        /// （Linqによるより高度な操作をえるようにこのメソッドを定義しておく）
        /// </summary>
        public IEnumerator<T> GetEnumerator() => _queue.GetEnumerator();

        // IEnumerator の明示的な実装
        IEnumerator IEnumerable.GetEnumerator() => _queue.GetEnumerator();
    }
}
